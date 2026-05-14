
import type { ApiResponse } from '../types/User';

class UserService {
    static async login(credentials: { login: string; password: string }): Promise<ApiResponse> {
        // TODO: реальный запрос к серверу
        // const response = await fetch('/api/login', {
        //   method: 'POST',
        //   headers: { 'Content-Type': 'application/json' },
        //   body: JSON.stringify(credentials),
        // });
        // return response;

        // Заглушка для демонстрации
        return new Promise((resolve) => {
            setTimeout(() => {
                if (credentials.login === 'demo' && credentials.password === '123456') {
                    resolve({
                        ok: true,
                        status: 200,
                        data: { token: 'fake-token', user: credentials.login }
                    });
                } else {
                    resolve({
                        ok: false,
                        status: 404,
                        error: 'Неверный логин или пароль'
                    });
                }
            }, 500);
        });
    }

    static async register(userData: { login: string; password: string }): Promise<ApiResponse> {
        // TODO: реальный запрос к серверу
        // const response = await fetch('/api/register', {
        //   method: 'POST',
        //   headers: { 'Content-Type': 'application/json' },
        //   body: JSON.stringify(userData),
        // });
        // return response;

        // Заглушка
        console.log('Регистрация пользователя:', userData);
        return new Promise((resolve) => {
            setTimeout(() => {
                resolve({
                    ok: true,
                    status: 200,
                    data: { message: 'Регистрация успешна' }
                });
            }, 500);
        });
    }
}

export default UserService;