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


type ViewType = 'constructor' | 'schema';

function App() {
    const [currentView, setCurrentView] = useState<ViewType>('constructor');
    const [isAuthenticated, setIsAuthenticated] = useState<boolean>(() => {
        // Проверяем, есть ли пользователь в localStorage при загрузке
        return !!localStorage.getItem('user');
    });

    const handleLoginSuccess = () => {
        setIsAuthenticated(true);
    };

   /* const handleLogout = () => {
        localStorage.removeItem('user');
        setIsAuthenticated(false);
    };*/

    // Если не авторизован - показываем страницу авторизации
    if (!isAuthenticated) {
        return <AuthView onLoginSuccess={handleLoginSuccess} />;
    }

    // Если авторизован - показываем основной контент
    return (
        <div className="app">
            <Navigation currentView={currentView} onViewChange={setCurrentView} />
            <div className="content">
                {currentView === 'constructor' && <ConstructorView />}
                {currentView === 'schema' && <SchemaConstructorView />}
            </div>
        </div>
    );


}

export default App
