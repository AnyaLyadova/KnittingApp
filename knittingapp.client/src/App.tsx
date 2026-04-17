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

type ViewType = 'constructor' | 'schema';

function App() {
    const [currentView, setCurrentView] = useState<ViewType>('constructor');

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
