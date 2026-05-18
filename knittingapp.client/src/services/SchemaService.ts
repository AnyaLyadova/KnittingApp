import apiClient from './apiClient'
import type { Schema } from "../types/Schema"

export const SchemaService={

    // Получить схему по ID (GET /api/schema/{schemaId})
    async getSchemaById(schemaId: string): Promise<Schema> {
        const response = await apiClient.get(`/schema/${schemaId}`);
        return response.data;
    },

    // Получить все схемы (GET /api/schema/all)
    async getAllSchemas(): Promise<Schema[]> {
        const response = await apiClient.get('/schema/all');
        return response.data;
    },

    // Изменить цвет конкретной петли (PUT /api/schema/color)
    async changeLoopColor(id: string,m: number, n: number, colorCode: string): Promise<void> {
        const response = await apiClient.put(`/schema/${ id } /color`, null, {
            params: {
                m: m,
                n: n,
                colorCode: colorCode
            }
        });
        return response.data;
    },

    // Изменить тип конкретной петли (PUT /api/schema/type)
    async changeLoopType(m: number, n: number, type: string): Promise<void> {
        const response = await apiClient.put('/schema/type', null, {
            params: {
                m: m,
                n: n,
                type: type
            }
        });
        return response.data;
    },

    // Перекрасить схему (PUT /api/schema/color/{colorCode})
    async colorSchema(colorCode: string): Promise<void> {
        const response = await apiClient.put(`/schema/color/${colorCode}`);
        return response.data;
    },
    // Создать новую схему (POST /api/schema/create)
    async createSchema(m: number, n: number, name: string): Promise<Schema> {
        const response = await apiClient.post('/schema/create', null, {
            params: { m, n, name }
        });
        return response.data;
    },

    //Отправить картинку схемы
    async setSchemaImage(schemaId: string, schemaImage: Blob): Promise<void> {
        const formData = new FormData();
        formData.append('schemaImage', schemaImage, `${schemaId}.png`);

        await apiClient.post(`/schema/${schemaId}/image`, formData, {
            headers: {
                'Content-Type': 'multipart/form-data',
            },
        });
    },

}