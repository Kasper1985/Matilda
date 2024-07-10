import { app, BrowserWindow } from 'electron';
import { fileURLToPath } from 'url';
import * as path from 'path';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
const isDev = process.env.IS_DEV === 'true';

const createWindow = () => {
    // Create the browser window.
    const mainWindow = new BrowserWindow({
        width: 1280,
        height: 800,
        webPreferences: {
            nodeIntegration: true,
            worldSafeExecution: true,
            contextIsolation: true,
            webSecurity: false,
            preload: path.join(__dirname, 'preload.cjs'),
        }
    })

    // and load the app.
    if (isDev) {
        //mainWindow.removeMenu();
        mainWindow.loadURL('http://localhost:3000')
        mainWindow.webContents.openDevTools();
    }
    else {
        mainWindow.removeMenu();
        mainWindow.loadFile('index.html')
    }
}

// This method will be called when Electron has finished
// initialization and is ready to create browser windows.
// Some APIs can only be used after this event occurs.
app.whenReady().then(() => {
    createWindow()

    app.on('activate', () => {
        // On macOS it'S common to re-create a window in the app when the
        // dock icon is clicked and there are no other windows open.
        if (BrowserWindow.getAllWindows().length === 0) createWindow()
    })
})

// Quit when all windows are closed, except on macOS. There, it's common
// for applications and their menu bar to stay active until the user quits
// explicitly with Cmd + Q.
app.on('window-all-closed', () => {
    if (process.platform !== 'darwin') app.quit()
})

// In this file you can include the rest of your app's specific main process code.
// You can also put them in separate files and import them here.