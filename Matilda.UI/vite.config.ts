import { defineConfig, loadEnv } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'path';

// https://vitejs.dev/config/
export default defineConfig(({mode}) => {
  const pathToEnv = path.resolve(process.cwd(), 'environment');
  process.env = {...process.env, ...loadEnv(mode, pathToEnv, ['', '.local', '.secrets.local'])}; 
  return {
    define: {
      'process.env': process.env
    },
    plugins: [react()],
    base: '',
    server: {
      port: 3000,
    }
  }
})
