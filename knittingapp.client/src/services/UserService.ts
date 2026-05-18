import apiClient from './apiClient'
import type { ApiResponse } from '../types/User';
import axios from 'axios';


class UserService {
    static async login(credentials: { login: string; password: string }): Promise<ApiResponse> {
        try {
            const response = await apiClient.get('/user/login', {
                params: {
                    username: credentials.login,
                    password: credentials.password
                }
            });

            localStorage.setItem('access_token', response.data);

            return {
                ok: true,
                status: 200,
                data: { token: response.data, user: credentials.login }
            };
        } catch (error: unknown) {
            // Проверяем, что это axios ошибка
            if (axios.isAxiosError(error)) {
                if (error.response?.status === 401) {
                    return {
                        ok: false,
                        status: 401,
                        error: 'Неверный логин или пароль'
                    };
                }

                return {
                    ok: false,
                    status: error.response?.status || 500,
                    error: error.response?.data?.message || 'Ошибка сервера'
                };
            }

            // Если это не axios ошибка
            return {
                ok: false,
                status: 500,
                error: 'Неизвестная ошибка'
            };
        }
    }

    static async register(userData: { login: string; password: string }): Promise<ApiResponse> {
        try {
            await apiClient.post('/user/register', null, {
                params: {
                    username: userData.login,
                    password: userData.password
                }
            });

            return {
                ok: true,
                status: 200,
                data: { message: 'Регистрация успешна' }
            };
        } catch (error: unknown) {
            if (axios.isAxiosError(error)) {
                return {
                    ok: false,
                    status: error.response?.status || 500,
                    error: error.response?.data?.message || 'Ошибка регистрации'
                };
            }

            return {
                ok: false,
                status: 500,
                error: 'Неизвестная ошибка'
            };
        }
    }

    static logout(): void {
        localStorage.removeItem('access_token');
    }
}


export default UserService;