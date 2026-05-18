import axios from 'axios';

// создаём и настраиваем клиент
export const apiClient = axios.create({
    baseURL: '/api',
    timeout: 10000,
    headers: {
        'Content-Type': 'application/json',
    },
});

//  для добавления токена в каждый запрос
apiClient.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('access_token');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        };
        return config;
    },
    (error) => Promise.reject(error)
);

// перехватчики ответов
apiClient.interceptors.response.use(
    (response) => {
        console.log(` ${response.status} ${response.config.url}`);
        return response;
    },
    (error) => {
        if (error.response?.status === 401) {
            window.location.href = '/login';
            console.log('Сессия истекла');
        }
        return Promise.reject(error);
    }

);

//для обработки 401
apiClient.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error.response?.status === 401) {
            localStorage.removeItem('access_token');
            window.location.href = '/login';
        }
        return Promise.reject(error);
    }
);

export default apiClient