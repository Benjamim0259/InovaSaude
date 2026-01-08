import React, { createContext, useContext, useState, useCallback } from 'react';
import { Loading } from '../components/Loading/Loading';

interface LoadingContextType {
  isLoading: boolean;
  setIsLoading: (loading: boolean) => void;
  withLoading: <T,>(promise: Promise<T>, duration?: number) => Promise<T>;
}

const LoadingContext = createContext<LoadingContextType | undefined>(undefined);

export const LoadingProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [isLoading, setIsLoading] = useState(false);

  const withLoading = useCallback(async <T,>(
    promise: Promise<T>, 
    duration: number = 500
  ): Promise<T> => {
    setIsLoading(true);
    try {
      const result = await promise;
      
      // Garante que o loading mínimo seja exibido
      await new Promise(resolve => setTimeout(resolve, duration));
      
      return result;
    } finally {
      setIsLoading(false);
    }
  }, []);

  return (
    <LoadingContext.Provider value={{ isLoading, setIsLoading, withLoading }}>
      {children}
      {isLoading && <Loading fullScreen message="Aguardando resposta do servidor..." />}
    </LoadingContext.Provider>
  );
};

export const useLoading = () => {
  const context = useContext(LoadingContext);
  if (!context) {
    throw new Error('useLoading deve ser usado dentro de LoadingProvider');
  }
  return context;
};
