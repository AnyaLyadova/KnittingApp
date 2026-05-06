import apiClient from './apiClient'
import type { NeckParts, SleeveRollParts, ArmholeParts,Model } from '../types/Model';
import type { LoopMap } from '../types/Schema';


export const ModelService = {
        // Получить словарь частей шеи
        async getNeckParts(): Promise<NeckParts> {
            const response = await apiClient.get('/constructor/neckparts');
            return response.data;  // 
        },

        // Получить словарь скосов рукава
        async getSleeveRollParts(): Promise<SleeveRollParts> {
            const response = await apiClient.get('/constructor/sleeverollparts');
            return response.data;
        },

        // Получить словарь выемок под рукав
        async getArmholeParts(): Promise<ArmholeParts> {
            const response = await apiClient.get('/constructor/armholeparts');
            return response.data;
        },

    // Создать модель (отправка на сервер)
    async createModel(data: Model): Promise<Model> {
        // Формируем query параметры
        const params = new URLSearchParams();
        params.append('name', data.name);

        // Каждую часть добавляем отдельным параметром с одним именем 'stringParts'
        data.parts.forEach(part => {
            params.append('stringParts', part);
        });

        const response = await apiClient.post('/constructor/create', null, {
            params: params
        });
        return response.data;
    },

    // Получить матрицу петель
    async getLoopMap(draftType:string): Promise<LoopMap> {
        const response = await apiClient.get('/constructor/loopMap', 
            {
                params: {
                    draftType: draftType
                }
            });
        return response.data;
    },
   
    };