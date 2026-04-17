import apiClient from './apiClient'
import type { Draft } from '../types/Draft';


export const DraftService = {
    async getDraft(): Promise<Draft> {
        const response = await apiClient.get<Draft>('/constructor/draft');
        return response.data;
    },
};