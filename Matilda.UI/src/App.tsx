import React, { useState } from 'react';
import { Box } from '@mui/material';
import { makeStyles } from 'tss-react/mui';
import Navigation from './components/Navigation';
//import DarkModeIconButton from '@mui/icons-material/DarkMode';
//import LightModeIconButton from '@mui/icons-material/LightMode';
import Chat from './components/Chat';
import './App.css';

const useStyles = makeStyles()(() => ({
    main: {
        width: '100%',
        height: '100vh',
        padding: '2em',
        display: 'flex'
    },
    navigation: {
        width: '25%',
        height: '100%',
        paddingRight: '10px',
        flexShrink: 0
    },
    chat: {
        width: '75%',
        height: '100%',
    },
    version: {
        fontSize: '0.6em',
        color: '#888',
        display: 'flex',
        alignItems: 'end',
    }
}));

function App() {
    //const [colorMode, setColorMode] = useState<'dark' | 'light'>('light');
    const [selectedChatId, setSelectedChatId] = useState<string>('');
    const { classes } = useStyles();

    return (
        <Box className={classes.main}>
            <Box className={classes.navigation}>
                <Navigation onSelectChat={(id: string) => setSelectedChatId(id)}/>
            </Box>
            <Box className={classes.chat}>
                <Chat chatId={selectedChatId}/>
            </Box>
            {/*<Stack direction="row" spacing={1}>
                <IconButton aria-label="color mode" color="primary" onClick={() => colorMode === 'dark' ? setColorMode('light') : setColorMode('dark')}>
                { colorMode === 'dark' ? <DarkModeIconButton/> : <LightModeIconButton/> }
                </IconButton>
                <div className={classes.version}>© Varshavskyy | 0.0.1</div>
            </Stack>*/}

        </Box>
    )
}

export default React.memo(App);
