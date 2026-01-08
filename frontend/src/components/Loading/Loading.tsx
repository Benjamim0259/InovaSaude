import React from 'react';
import './Loading.css';

interface LoadingProps {
  fullScreen?: boolean;
  message?: string;
}

export const Loading: React.FC<LoadingProps> = ({ 
  fullScreen = false, 
  message = 'Carregando...' 
}) => {
  const containerClass = fullScreen 
    ? 'fixed inset-0 bg-white bg-opacity-90 z-50' 
    : 'absolute inset-0 bg-white bg-opacity-50';

  return (
    <div className={`flex items-center justify-center ${containerClass}`}>
      <div className="flex flex-col items-center gap-4">
        {/* Spinner animado */}
        <div className="relative w-16 h-16">
          <div className="loading-spinner"></div>
        </div>
        
        {/* Texto com pulsação */}
        <div className="text-center">
          <p className="text-gray-700 font-medium text-sm loading-text">
            {message}
          </p>
          <div className="flex justify-center gap-1 mt-2">
            <span className="loading-dot"></span>
            <span className="loading-dot" style={{ animationDelay: '0.2s' }}></span>
            <span className="loading-dot" style={{ animationDelay: '0.4s' }}></span>
          </div>
        </div>
      </div>
    </div>
  );
};

export const SkeletonCard: React.FC = () => (
  <div className="bg-white overflow-hidden shadow rounded-lg animate-pulse">
    <div className="p-5">
      <div className="flex items-center">
        <div className="flex-shrink-0">
          <div className="w-12 h-12 bg-gray-200 rounded-full"></div>
        </div>
        <div className="ml-5 w-0 flex-1">
          <div className="h-4 bg-gray-200 rounded w-24 mb-2"></div>
          <div className="h-6 bg-gray-200 rounded w-16"></div>
        </div>
      </div>
    </div>
  </div>
);

export const SkeletonTable: React.FC = () => (
  <div className="bg-white shadow overflow-hidden sm:rounded-md animate-pulse">
    <ul className="divide-y divide-gray-200">
      {[...Array(5)].map((_, i) => (
        <li key={i} className="px-4 py-4 sm:px-6">
          <div className="flex items-center justify-between">
            <div className="flex-1">
              <div className="h-4 bg-gray-200 rounded w-1/3 mb-2"></div>
              <div className="h-3 bg-gray-100 rounded w-1/4"></div>
            </div>
            <div className="h-4 bg-gray-200 rounded w-1/6"></div>
          </div>
        </li>
      ))}
    </ul>
  </div>
);
