interface NavigationProps {
    currentView: 'constructor' | 'schema';
    onViewChange: (view: 'constructor' | 'schema') => void;
}

const Navigation: React.FC<NavigationProps> = ({ currentView, onViewChange }) => {
    return (
        <nav className="navigation">
            <button
                className={currentView === 'constructor' ? 'active' : ''}
                onClick={() => onViewChange('constructor')}
            >
                Конструктор моделей
            </button>
            <button
                className={currentView === 'schema' ? 'active' : ''}
                onClick={() => onViewChange('schema')}
            >
               Создание схемы
            </button>
        </nav>
    );
};

export default Navigation;