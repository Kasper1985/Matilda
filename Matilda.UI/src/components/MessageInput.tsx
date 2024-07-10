import React, { useState } from 'react';
import { Box, IconButton, TextField } from '@mui/material';
import { Send as SendIcon } from '@mui/icons-material';
import { useMessageInput } from '../contexts/MessageInputContextProvider';
import { messageInputStyles } from './styles/MessageInput.styles';

export function MessageInput() {
    const {setMessageInput} = useMessageInput();
    const [text, setText] = useState<string>('');
    const {classes} = messageInputStyles();

    function handleKeyDown(event: React.KeyboardEvent<HTMLDivElement>) {
        if (event.key === 'Enter' && !event.shiftKey) {
            event.preventDefault();
            sendMessage();
        }
    }

    function sendMessage() {
        setMessageInput({ message: text });
        setText('');
    }

    return (
        <Box className={classes.main}>
            <TextField 
                multiline
                maxRows={2}
                placeholder='Type message here...'
                variant='standard'
                fullWidth
                value={text}
                onChange={(e) => setText(e.target.value)}
                onKeyDown={handleKeyDown}/>
            <IconButton onClick={sendMessage}>
                <SendIcon />
            </IconButton>
        </Box>
    )
}