
import apiClient from './apiClient'
import type { Model, Measures } from '../types/Model';
import type { LoopMap } from '../types/Schema';


export const ConstructorService = {

    //получить список всех моделей
    async getModels(): Promise<Model[]> {
        const response = await apiClient.get('/constructor/models');
        return response.data;
    },

    //выбор модели
    async chooseModel(modelId: string): Promise<Model> {
        const response = await apiClient.get(`/constructor/model/${modelId}`);
        return response.data;
    },

    //получение списка мерок
      async getMeasures(): Promise<Measures> {
          const response = await apiClient.get('/constructor/measures');
        return response.data;  
    },

    //инициализация мерок
    async sendMeasures(measures: Measures,  // в тело
        height: number,     // в query
        width: number,      // в query
        loopInHeight: number, // в query
        loopInWidth: number   // в query
    ): Promise<void> {
        const response = await apiClient.put('/constructor/measures/initialize', measures, {
            params: {
                height: height,
                width: width,
                loopInHeight: loopInHeight,
                loopInWidth: loopInWidth
            }
        });
        return response.data;
    },


    //раскрашивание схемы
    async colorLoopMap(mIndexes: number[], nIndexes: number[], colors: string[]): Promise<LoopMap> {
        const response = await apiClient.put(`/constructor/color`, {
            mIndexes: mIndexes,
            nIndexes: nIndexes,
            colors: colors
        });
        return response.data;
    },

}