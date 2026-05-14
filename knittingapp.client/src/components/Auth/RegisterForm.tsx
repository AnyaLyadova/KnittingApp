import React, { useState } from 'react';
import type { ChangeEvent as ReactChangeEvent } from 'react';
import UserService from '../../services/UserService';

interface RegisterFormProps {
    onSwitch: () => void;
    onSuccess: () => void;
}

interface FormErrors {
    login: string;
    password: string;
}

const RegisterForm: React.FC<RegisterFormProps> = ({ onSwitch, onSuccess }) => {
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

        const response = await UserService.register(formData);

        if (response.ok && response.status === 200) {
            const authResponse = await UserService.login(formData);
            if (authResponse.ok && authResponse.data) {
                localStorage.setItem('user', JSON.stringify(authResponse.data));
                onSuccess();
            }
        } else {
            setServerError('Ошибка регистрации, попробуйте другой логин');
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
            color: '#333',
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
            color: '#555',
        },
        hint: {
            fontSize: '12px',
            color: '#999',
            marginBottom: '2px',
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
            color: '#fff',
            backgroundColor: '#2ecc71',
            border: 'none',
            borderRadius: '4px',
            cursor: 'pointer',
            marginTop: '10px',
        },
        switch: {
            textAlign: 'center' as const,
            marginTop: '20px',
            fontSize: '14px',
            color: '#666',
        },
        linkButton: {
            background: 'none',
            border: 'none',
            color: '#2ecc71',
            cursor: 'pointer',
            fontSize: '14px',
            textDecoration: 'underline',
        },
    };

    return (
        <div style={styles.block}>
            <h2 style={styles.title}>Регистрация</h2>
            <form onSubmit={handleSubmit} style={styles.form}>
                <div style={styles.field}>
                    <label style={styles.label}>Логин</label>
                    <div style={styles.hint}>придумайте уникальный логин</div>
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
                    <div style={styles.hint}>придумайте надежный пароль (не менее 6 символов)</div>
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
                    Регистрация
                </button>
            </form>

            <div style={styles.switch}>
                <span>Уже есть аккаунт? </span>
                <button onClick={onSwitch} style={styles.linkButton}>
                    Войти
                </button>
            </div>
        </div>
    );
};

export default RegisterForm;