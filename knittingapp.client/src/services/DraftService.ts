import apiClient from './apiClient'
import type { Draft } from '../types/Draft';
import type { Point } from '../types/Draft';


export const DraftService = {
    async createDrafts(): Promise<Draft> {
        const response = await apiClient.get<Draft>('/constructor/createDrats');
        return response.data;  //возвращается frontDraft
    },

    async getFrontDraft(modelId:string): Promise<Draft> {
        const response = await apiClient.get<Draft>(`/constructor/${modelId}/frontDraft`);
        return response.data;  //возвращается frontDraft
    },

    async getBackDraft(modelId: string): Promise<Draft> {
        const response = await apiClient.get<Draft>(`/constructor/${modelId}/backDraft`);
        return response.data;  //возвращается frontDraft
    },

    async getSleeveDraft(modelId: string): Promise<Draft> {
        const response = await apiClient.get<Draft>(`/constructor/${modelId}/sleeveDraft`);
        return response.data;  //возвращается frontDraft
    },

    async movePoint(
        draftId:string,
        movingPoint: Point,  // старая точка (с исходными координатами)
        newX: number,        // новая X координата
        newY: number,        // новая Y координата
        leftPoint: Point,    // левая опорная точка
        rightPoint: Point,    // правая опорная точка
        draftType:string
    ): Promise<Draft> {
        const response = await apiClient.post<Draft>(`/constructor/${draftId}/move`, {
            movingPoint: movingPoint,
            leftPoint: leftPoint,
            rightPoint: rightPoint
        }, {
            params: {
                newX: newX,
                newY: newY,
                draftType: draftType
            }
        });
        return response.data;
    }
   
};