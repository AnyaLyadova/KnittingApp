import React, { useRef, useEffect } from 'react';
//import { StayCanvas, Point as StayPoint, Line as StayLine } from 'react-stay-canvas';
//import type { StayTools }from 'react-stay-canvas';
import type { Draft } from '../../types/Draft';

interface DraftFieldProps {
    draft: Draft|null;
    width?: number;
    height?: number;
}


export function DraftField({ draft, width = 800, height = 600 }: DraftFieldProps) {
    const canvasRef = useRef<HTMLCanvasElement>(null);

    const handleCanvasClick = (event: React.MouseEvent<HTMLCanvasElement>) => {
        if (!draft || !draft.draft || draft.draft.length === 0) return;

        const canvas = canvasRef.current;
        if (!canvas) return;

        const rect = canvas.getBoundingClientRect();
        const clickX = event.clientX - rect.left;
        const clickY = event.clientY - rect.top;

        const points = draft.draft;
        // Находим границы
        let minX = Infinity, maxX = -Infinity, minY = Infinity, maxY = -Infinity;
        for (const point of points) {
            if (point.x < minX) minX = point.x;
            if (point.x > maxX) maxX = point.x;
            if (point.y < minY) minY = point.y;
            if (point.y > maxY) maxY = point.y;
        }

        // Отступы (40px с каждой стороны)
        const padding = 40;
        const rangeX = maxX - minX;
        const rangeY = maxY - minY;

        // Вычисляем масштаб
        const scaleX = (width - padding * 2) / rangeX;
        const scaleY = (height - padding * 2) / rangeY;
        const scale = Math.min(scaleX, scaleY);

        // Смещение
        const offsetX = padding - minX * scale;
        const offsetY = padding - minY * scale;

        const radius = 100;
        let clickedPointIndex = -1;
        let minDistance = Infinity;

        for (let i = 0; i < points.length; i++) {
            const point = points[i];
            const canvasX = point.x * scale + offsetX;
            const canvasY = canvas.height - (point.y * scale + offsetY);

            const distance = Math.sqrt(
                Math.pow(canvasX - clickX, 2) +
                Math.pow(canvasY - clickY, 2)
            );

            if (distance < radius && distance < minDistance) {
                minDistance = distance;
                clickedPointIndex = i;
            }
        }

        if (clickedPointIndex !== -1) {
            const point = points[clickedPointIndex];
            console.log('=== КЛИК ПО ТОЧКЕ ===');
            console.log('Индекс в массиве:', clickedPointIndex);
            console.log('Координаты с бэка (оригинальные):', { x: point.x, y: point.y });
            console.log('Часть:', point.part || 'не указана');
            console.log('=====================');
        } else {
            console.log('Клик мимо точек');
        }
    };

    useEffect(() => {
        const canvas = canvasRef.current;
        if (!canvas) return;

        const ctx = canvas.getContext('2d');
        if (!ctx) return;

        ctx.clearRect(0, 0, width, height);
        ctx.fillStyle = '#f9f9f9';
        ctx.fillRect(0, 0, width, height);

        if (!draft || !draft.draft || draft.draft.length === 0) {
            ctx.fillStyle = '#999';
            ctx.font = '24px sans-serif';
            ctx.textAlign = 'center';
            ctx.textBaseline = 'middle';
            ctx.fillText('Выберите модель', width / 2, height / 2);
            return;
        }

        const points = draft.draft;

        // Находим границы всех точек
        let minX = Infinity, maxX = -Infinity, minY = Infinity, maxY = -Infinity;
        for (const point of points) {
            if (point.x < minX) minX = point.x;
            if (point.x > maxX) maxX = point.x;
            if (point.y < minY) minY = point.y;
            if (point.y > maxY) maxY = point.y;
        }

        // Отступы от краёв canvas (40px = примерно 1.5 см при стандартном DPI)
        const padding = 40;
        const rangeX = maxX - minX;
        const rangeY = maxY - minY;

        // Вычисляем масштаб (используем минимальный, чтобы сохранить пропорции)
        const scaleX = (width - padding * 2) / rangeX;
        const scaleY = (height - padding * 2) / rangeY;
        const scale = Math.min(scaleX, scaleY);

        // Вычисляем смещение, чтобы чертёж оказался по центру
        const offsetX = padding - minX * scale;
        const offsetY = padding - minY * scale;

   
        // Рисуем линии
        ctx.strokeStyle = '#000000';
        ctx.lineWidth = 2;

        for (let i = 0; i < points.length - 1; i++) {
            const p1 = points[i];
            const p2 = points[i + 1];


            const x1 = p1.x * scale + offsetX;
            const y1 = canvas.height - (p1.y * scale + offsetY);
            const x2 = p2.x * scale + offsetX;
            const y2 = canvas.height - (p2.y * scale + offsetY);

            ctx.beginPath();
            ctx.moveTo(x1, y1);
            ctx.lineTo(x2, y2);
            ctx.stroke();
        }

        // Рисуем точки (радиус 3)
        for (const point of points) {
            const x = point.x * scale + offsetX;
            const y = canvas.height - (point.y * scale + offsetY);

            ctx.beginPath();
            ctx.fillStyle = '#3b82f6';
            ctx.arc(x, y, 3, 0, 2 * Math.PI);
            ctx.fill();

            ctx.beginPath();
            ctx.strokeStyle = '#ffffff';
            ctx.lineWidth = 1;
            ctx.arc(x, y, 3, 0, 2 * Math.PI);
            ctx.stroke();
        }

    }, [draft, width, height]);

    return (
        <canvas
            ref={canvasRef}
            width={width}
            height={height}
            onClick={handleCanvasClick}                    
            style={{
                border: '1px solid #ccc',
                backgroundColor: '#f9f9f9',
                borderRadius: '8px',
                cursor: 'pointer'
            }}
        />
    );
}

export default DraftField;