import React, { useEffect, useState } from 'react';
import { Box } from '@mui/material';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import { MessageInputProvider } from './contexts/MessageInputContextProvider';
import { appStyles } from './components/styles/App.styles';
import { Navigation, MessageInput, ChatHistory } from './components';
import './App.css';

const darkTheme = createTheme({
    palette: {
        mode: 'dark',
    },
})

function App() {
    const [selectedChatId, setSelectedChatId] = useState<string>('');
    const { classes } = appStyles();

    useEffect(() => {
        
    }, []);

    return (
        <ThemeProvider theme={darkTheme}>
            <Box className={classes.main}>
                <Box className={classes.navigation}>
                    <Navigation onSelectChat={(id: string) => setSelectedChatId(id)}/>
                </Box>
                <Box className={classes.chat}>
                    <MessageInputProvider>
                        <ChatHistory chatId={selectedChatId}/>
                        <MessageInput />
                    </MessageInputProvider>
                </Box>
            </Box>
        </ThemeProvider>
    )
}

export default React.memo(App);
