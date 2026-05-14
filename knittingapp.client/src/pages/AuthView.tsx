
import React, { useState } from 'react';
import LoginForm from '../components/Auth/LoginForm';
import RegisterForm from '../components/Auth/RegisterForm';

interface AuthViewProps {
    onLoginSuccess: () => void;
}

const AuthView: React.FC<AuthViewProps> = ({ onLoginSuccess }) => {
    const [isLoginMode, setIsLoginMode] = useState<boolean>(true);

    return (
        <div>
            {isLoginMode ? (
                <LoginForm
                    onSwitch={() => setIsLoginMode(false)}
                    onSuccess={onLoginSuccess}
                />
            ) : (
                <RegisterForm
                    onSwitch={() => setIsLoginMode(true)}
                    onSuccess={onLoginSuccess}
                />
            )}
        </div>
    );
};

export default AuthView;