import React, { useState } from 'react';
import type { ChangeEvent as ReactChangeEvent } from 'react';
import UserService from '../../services/UserService';

interface LoginFormProps {
    onSwitch: () => void;
    onSuccess: () => void;
}

interface FormErrors {
    login: string;
    password: string;
}

const LoginForm: React.FC<LoginFormProps> = ({ onSwitch, onSuccess }) => {
    const [formData, setFormData] = useState({ login: '', password: '' });
    const [errors, setErrors] = useState<FormErrors>({ login: '', password: '' });
    const [serverError, setServerError] = useState<string>('');

    const validateLogin = (login: string): string => {
        const regex = /^[a-zA-Z0-9_-]+$/;
        if (!login) return 'Логин обязателен';
        if (!regex.test(login)) return 'Логин содержит недопустимые символы (разрешены: буквы, цифры, _, -)';
        return '';
    };

    const validatePassword = (password: string): string => {
        if (!password) return 'Пароль обязателен';
        if (password.length < 6) return 'Пароль должен быть не менее 6 символов';
        return '';
    };

    const handleChange = (e: ReactChangeEvent<HTMLInputElement>): void => {
        const { name, value } = e.target;
        setFormData({ ...formData, [name]: value });
        setServerError('');

        if (name === 'login') {
            setErrors({ ...errors, login: validateLogin(value) });
        } else if (name === 'password') {
            setErrors({ ...errors, password: validatePassword(value) });
        }
    };

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>): Promise<void> => {
        e.preventDefault();

        const loginError = validateLogin(formData.login);
        const passwordError = validatePassword(formData.password);

        if (loginError || passwordError) {
            setErrors({ login: loginError, password: passwordError });
            return;
        }

        const response = await UserService.login(formData);

        if (response.ok && response.status === 200 && response.data) {
            localStorage.setItem('user', JSON.stringify(response.data));
            onSuccess();
        } else if (response.status === 404 && response.error) {
            setServerError(response.error);
        } else {
            setServerError('Ошибка сервера, попробуйте позже');
        }
    };

    const styles: { [key: string]: React.CSSProperties } = {
        block: {
            maxWidth: '400px',
            margin: '50px auto',
            padding: '30px',
            borderRadius: '8px',
            boxShadow: '0 2px 10px rgba(0,0,0,0.1)',
            backgroundColor: '#fff',
        },
        title: {
            textAlign: 'center' as const,
            marginBottom: '20px',
            color: 'white',
            fontWeight: 'bold'
        },
        form: {
            display: 'flex',
            flexDirection: 'column' as const,
            gap: '15px',
        },
        field: {
            display: 'flex',
            flexDirection: 'column' as const,
            gap: '5px',
        },
        label: {
            fontSize: '14px',
            fontWeight: '500',
            color: 'white',
        },
        input: {
            padding: '10px',
            fontSize: '16px',
            border: '1px solid #ddd',
            borderRadius: '4px',
            outline: 'none',
        },
        error: {
            fontSize: '12px',
            color: '#e74c3c',
        },
        serverError: {
            padding: '10px',
            backgroundColor: '#fee',
            border: '1px solid #fcc',
            borderRadius: '4px',
            color: '#e74c3c',
            fontSize: '14px',
            textAlign: 'center' as const,
        },
        button: {
            padding: '12px',
            fontSize: '16px',
            fontWeight: '600',
            color: 'white',
            backgroundColor: '#3498db',
            border: 'none',
            borderRadius: '4px',
            cursor: 'pointer',
            marginTop: '10px',
        },
        switch: {
            textAlign: 'center' as const,
            marginTop: '20px',
            fontSize: '14px',
            color: 'white',
        },
        linkButton: {
            background: 'none',
            border: 'none',
            color: 'white',
            cursor: 'pointer',
            fontSize: '14px',
            textDecoration: 'underline',
        },
    };

    return (
        <div className="login-form" style={styles.block}>
            <h2 style={styles.title}>Вход</h2>
            <form onSubmit={handleSubmit} style={styles.form}>
                <div style={styles.field}>
                    <label style={styles.label}>Логин</label>
                    <input
                        type="text"
                        name="login"
                        value={formData.login}
                        onChange={handleChange}
                        style={styles.input}
                    />
                    {errors.login && <span style={styles.error}>{errors.login}</span>}
                </div>

                <div style={styles.field}>
                    <label style={styles.label}>Пароль</label>
                    <input
                        type="password"
                        name="password"
                        value={formData.password}
                        onChange={handleChange}
                        style={styles.input}
                    />
                    {errors.password && <span style={styles.error}>{errors.password}</span>}
                </div>

                {serverError && <div style={styles.serverError}>{serverError}</div>}

                <button type="submit" style={styles.button}>
                    Вход
                </button>
            </form>

            <div style={styles.switch}>
                <span>Нет аккаунта? </span>
                <button onClick={onSwitch} style={styles.linkButton}>
                    Зарегистрироваться
                </button>
            </div>
        </div>
    );
};

export default LoginForm;