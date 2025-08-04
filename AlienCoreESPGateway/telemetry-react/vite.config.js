import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
    base: '/react/',        // the URL prefix your Blazor page will use
    build: {
        outDir: '../wwwroot/react',   // emits files into Blazor’s wwwroot/react
        assetsDir: 'assets',          // so scripts land in wwwroot/react/assets
        emptyOutDir: true,
        rollupOptions: {
            output: {
                // force stable filenames (no hashes)
                entryFileNames: 'assets/index.js',
                chunkFileNames: 'assets/[name].js',
                assetFileNames: 'assets/[name][extname]'
            }
        }
    },
    plugins: [react()]
});