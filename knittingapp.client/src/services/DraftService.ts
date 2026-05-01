import apiClient from './apiClient'
import type { Draft } from '../types/Draft';
import type { Point } from '../types/Draft';


export const DraftService = {
    async createDrafts(): Promise<Draft> {
        const response = await apiClient.get<Draft>('/constructor/createDrats');
        return response.data;  //возвращается frontDraft
    },

    async getFrontDraft(): Promise<Draft> {
        const response = await apiClient.get<Draft>('/constructor/frontDraft');
        return response.data;  //возвращается frontDraft
    },

    async getBackDraft(): Promise<Draft> {
        const response = await apiClient.get<Draft>('/constructor/backDraft');
        return response.data;  //возвращается frontDraft
    },

    async getSleeveDraft(): Promise<Draft> {
        const response = await apiClient.get<Draft>('/constructor/sleeveDraft');
        return response.data;  //возвращается frontDraft
    },

    async movePoint(
        movingPoint: Point,  // старая точка (с исходными координатами)
        newX: number,        // новая X координата
        newY: number,        // новая Y координата
        leftPoint: Point,    // левая опорная точка
        rightPoint: Point,    // правая опорная точка
        draftType:string
    ): Promise<Draft> {
        const response = await apiClient.post<Draft>('/constructor/move', {
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