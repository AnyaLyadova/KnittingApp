import apiClient from './apiClient'
import type { Draft } from '../types/Draft';
import type { Point } from '../types/Draft';


export const DraftService = {
    async getDraft(): Promise<Draft> {
        const response = await apiClient.get<Draft>('/constructor/draft');
        return response.data;
    },

    async movePoint(
        movingPoint: Point,  // старая точка (с исходными координатами)
        newX: number,        // новая X координата
        newY: number,        // новая Y координата
        leftPoint: Point,    // левая опорная точка
        rightPoint: Point    // правая опорная точка
    ): Promise<Draft> {
        const response = await apiClient.post<Draft>('/constructor/move', {
            movingPoint: movingPoint,
            leftPoint: leftPoint,
            rightPoint: rightPoint
        }, {
            params: {
                newX: newX,
                newY: newY
            }
        });
        return response.data;
    }
   
};