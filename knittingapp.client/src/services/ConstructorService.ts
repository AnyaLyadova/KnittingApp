
import apiClient from './apiClient'
import type { Measures, Model } from '../types/Model';
import type { LoopMap } from '../types/Schema';
import type { Form } from '../types/Form'

export const ConstructorService = {

    //получить список всех моделей
    async getModels(): Promise<Form[]> {
        const response = await apiClient.get('/constructor/forms');
        return response.data;
    },

    //выбор модели
    async chooseModel(formId: string): Promise<Form> {
        const response = await apiClient.get(`/constructor/form/${formId}`);
        return response.data;
    },

    //получение списка мерок
      async getMeasures(): Promise<Measures> {
          const response = await apiClient.get('/constructor/measures');
        return response.data;  
    },

    //инициализация мерок
    async sendMeasures(formId:string, modelName:string, measures: Measures,  // в тело
        height: number,     // в query
        width: number,      // в query
        loopInHeight: number, // в query
        loopInWidth: number   // в query
    ): Promise<Model> {
        const response = await apiClient.put(`/constructor/model/${formId}/initialize`, measures, {
            params: {
                modelName: modelName,
                height: height,
                width: width,
                loopInHeight: loopInHeight,
                loopInWidth: loopInWidth
            }
        });
        return response.data;
    },


    //раскрашивание схемы
    async colorLoopMap(modelId: string, mIndexes: number[], nIndexes: number[], colors: string[], draftType: string): Promise<LoopMap> {
        const response = await apiClient.put(`/constructor/${modelId} / color`, {
            mIndexes: mIndexes,
            nIndexes: nIndexes,
            colors: colors
        },
            {params: {
            draftType: draftType
            }}
        );
        return response.data;
    },

    //раскрашивание всей схемы
    async colorAllLoopMap(modelId: string, color: string, draftType: string): Promise<LoopMap> {
        const response = await apiClient.put(`/constructor/${modelId}/color/all`,null,
            {
                params: {
                    color:color,
                    draftType: draftType
                }
            }
        );
        return response.data;
    },

}