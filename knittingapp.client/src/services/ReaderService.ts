// ReaderService.ts
import apiClient from './apiClient'; // путь к вашему apiClient

export interface ReaderProgress {
    progress: number;
    spentTime: string; // TimeSpan в виде строки "hh:mm:ss"
}

class ReaderService {
    // Получить LoopsReader по ID модели и типу чертежа
    async getLoopReader(modelId: string, type: string): Promise<string> {
        const response = await apiClient.get(`/account/${modelId}/reader`, {
            params: { type: type }
        });
        return response.data; // возвращает Guid readerId
    }

   /* // Получить текущую строку (список петель) по ID ридера
    async getCurrentString(readerId: string): Promise<[string[], string[]]> {
        const response = await apiClient.get(`/account/${readerId}`);
        return response.data; // возвращает (List<string>, List<string>)
    }*/

    async getCurrentString(readerId: string): Promise<{ segments: string[], colors: string[] }> {
        const response = await apiClient.get(`/account/${readerId}`);
        return response.data;
    }

    // Перейти к следующей строке
    async moveNext(readerId: string): Promise<void> {
        await apiClient.post(`/account/${readerId}/next`);
    }

    // Получить прогресс 
    async getProgress(readerId: string): Promise<number> {
        const response = await apiClient.get(`/account/${readerId}/progress`);
        return response.data;
    }

    // Получить затраченное время 
    async getSpentTime(readerId: string): Promise<string> {
        const response = await apiClient.get(`/account/${readerId}/time`);
        return response.data; // TimeSpan в формате "hh:mm:ss"
    }

    // Установить затраченное время
    async setSpentTime(readerId: string, time: string): Promise<void> {
        await apiClient.post(`/account/${readerId}/time`, null, {
            params: { time: time }
        });
    }
}

export default new ReaderService();