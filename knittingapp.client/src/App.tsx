/*import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from './assets/vite.svg'
import heroImg from './assets/hero.png'*/
import './App.css'
import { useState } from 'react';
//import Demo from './pages/Demo'
//import ModelCreationForm from './pages/Constructor/ModelCreationForm'
import ConstructorView from './pages/ConstructorView'
import SchemaConstructorView from './pages/SchemaConstructorView'
import Navigation from './components/Default/Navigation'
import AuthView from './pages/AuthView';
import AccountView from './pages/AccountView';


type ViewType = 'constructor' | 'schema'|'account';

function App() {
    const [currentView, setCurrentView] = useState<ViewType>('account');
    const [isAuthenticated, setIsAuthenticated] = useState<boolean>(() => {
        // Проверяем, есть ли пользователь в localStorage при загрузке
        return !!localStorage.getItem('access_token');
    });

    const handleLoginSuccess = () => {
        setIsAuthenticated(true);
    };

   /* const handleLogout = () => {
        localStorage.removeItem('user');
        setIsAuthenticated(false);
    };*/

    if (!isAuthenticated) {
        return <AuthView onLoginSuccess={handleLoginSuccess} />;
    }

    return (
        <div className="app">
            <Navigation currentView={currentView} onViewChange={setCurrentView} />
            <div className="content">
                {currentView === 'constructor' && <ConstructorView />}
                {currentView === 'schema' && <SchemaConstructorView />}
                {currentView === 'account' && <AccountView />}
            </div>
        </div>
    );


}

export default App
