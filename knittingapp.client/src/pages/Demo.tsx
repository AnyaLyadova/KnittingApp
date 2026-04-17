// 1. Импортируем всё необходимое из библиотеки
//    StayCanvas - главный компонент, который создает canvas
import { StayCanvas } from 'react-stay-canvas';

// 2. Создаем компонент Demo
//    В React компонент - это функция, которая возвращает то, что нужно отрисовать
export default function Demo() {
    // 3. Возвращаем JSX (это как HTML, только внутри JavaScript)
    //    StayCanvas - это и есть наш canvas
    return (
        // Этот div нужен только для оформления, можно и без него
        <div style={{ padding: '20px' }}>
            <h2>Мой первый canvas</h2>
            {/* 
        StayCanvas - главный компонент
        width / height - размеры canvas (обязательно!)
        className - CSS класс для стилизации (необязательно)
      */}
            <StayCanvas
                width={500}   // ширина в пикселях
                height={500}  // высота в пикселях
                className="border"  // добавит рамку (если в CSS есть класс .border)
                mode="instant"
            />
        </div>
    );
}