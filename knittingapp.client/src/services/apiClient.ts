import axios from 'axios';

// Создаём и настраиваем клиент ОДИН РАЗ
export const apiClient = axios.create({
    baseURL: '/api',
    timeout: 10000,
    headers: {
        'Content-Type': 'application/json',
    },
});

// Перехватчики запросов (можно добавить токен авторизации)
apiClient.interceptors.request.use(
    (config) => {
        // Например, добавить токен из localStorage
        const token = localStorage.getItem('token');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        console.log(` ${config.method?.toUpperCase()} ${config.url}`);
        return config;
    },
    (error) => Promise.reject(error)
);

// Перехватчики ответов
apiClient.interceptors.response.use(
    (response) => {
        console.log(` ${response.status} ${response.config.url}`);
        return response;
    },
    (error) => {
        if (error.response?.status === 401) {
            // Не авторизован - перенаправить на логин
            console.log('Сессия истекла');
        }
        return Promise.reject(error);
    }
);

export default apiClient