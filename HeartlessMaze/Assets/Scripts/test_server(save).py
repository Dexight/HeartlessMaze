import socket
import threading
import sys
import librosa
import onnxruntime
import json
import torch
import re
import numpy as np

model_path = sys.argv[1]  # Путь к модели
vocab_path = sys.argv[2]  # Путь к словарю токенов
ort_session = None
idx_to_token = None

stop_event = threading.Event()
server_socket = None

def handle_client(client_socket):
    try:
        while not stop_event.is_set():
            # Получение данных из Unity
            data = client_socket.recv(1024).decode('utf-8')
            
            if (data == "stop"):
                stop_event.set()
                client_socket.send("...stopping server...".encode('utf-8'))
                break
            else:
                try:
                    result = transcribe_audio_onnx(data)# распознавание
                    client_socket.send(result.encode('utf-8'))# Отправка результата обратно в Unity
                except (ConnectionResetError, BrokenPipeError, ConnectionError) as e:
                    print(f"Client disconnected: {e}", flush=True)
                    break
                except OSError as e:
                    print(f"Socket error: {e}", flush=True)
                    break
    except Exception as e:
        print(f"HandleClientError: {e}", flush=True)
    finally:
        client_socket.close()

def start_server():
    global server, stop_event, ort_session, idx_to_token
    try:
        #Debug проверка существования model файла
        try:
            with open(model_path, 'r') as mfile:
                print("Model file exists and can be readed", flush=True)
        except FileNotFoundError:
            print("Model file is not exists", flush=True)
        except PermissionError:
            print("Problems with permissions to read model file.", flush=True)

        ort_session = onnxruntime.InferenceSession(model_path)#Загрузка модели
        print("model was loaded succesfuly")

        #Debug проверка существования vocabular файла
        try:
            with open(vocab_path, 'r') as vfile:
                print("Token vocabular file exists and can be readed", flush=True)
                vocab = json.load(vfile)
                idx_to_token = {int(idx): token for token, idx in vocab.items()} # index -> token
                print("model was loaded succesfuly", flush=True)
        except FileNotFoundError:
            print("Token vocabular file is not exists", flush=True)
        except PermissionError:
            print("Problems with permissions to read token vocabular file.", flush=True)


        server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        server.bind(("127.0.0.1", 12345))  # Адрес и порт сервера
        server.listen(1)
        server.settimeout(1)# Таймаут для проверки stop_event

        print("...py-server ready...", flush=True)

        while not stop_event.is_set():
            try:
                client_sock, addr = server.accept()
                client_handler = threading.Thread(target=handle_client, args=(client_sock,))
                client_handler.daemon = True # остановится если завершён основной поток
                client_handler.start()
            except socket.timeout: # таймаут accept() для проверки флага остановки
                continue;
            except OSError as e:
                if stop_event.is_set():
                    break
                raise # другие ошибки прокидываем выше
    except Exception as e:
        print(f"ServerError: {e}", flush=True)
    finally:
        if (server):
            server.close()
            print("Server stopped", flush=True)
    
def prepare_input(audio_path):
    audio, sr = librosa.load(audio_path, sr=16000)  # Частота дискретизации 16 кГц
    audio = audio / np.max(np.abs(audio))# Нормализация
    inputs = np.expand_dims(audio, axis=0).astype(np.float32) # Преобразование аудио в формат, который ожидает модель
    return inputs

def transcribe_audio_onnx(audio_path):
    inputs = prepare_input(audio_path)
    
    ort_inputs = {ort_session.get_inputs()[0].name: inputs}
    ort_outs = ort_session.run(None, ort_inputs)
    
    logits = torch.tensor(ort_outs[0])
    predicted_ids = torch.argmax(logits, dim=-1)
    
    transcription = "".join([idx_to_token.get(idx.item(), "") for idx in predicted_ids[0] if idx_to_token.get(idx.item(), "") != "<pad>"])
    transcription = re.sub(r'(.)\1+', r'\1', transcription.replace('|', ' '))
    return transcription

if __name__ == "__main__":
    start_server()