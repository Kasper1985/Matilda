import React, { useState, useEffect, useRef } from 'react';
import { makeStyles } from 'tss-react/mui';
import { Box, IconButton, TextField } from '@mui/material';
import SendIcon from '@mui/icons-material/Send';
import { ChatMessage } from '../../api/models';
import { ApiUrls } from '../../api/apiUrls';

const useStyles = makeStyles()(() => ({
    main: {
        display: 'flex',
        flexDirection: 'column',
        height: '100%',
        gap: '10px',
    },
    chatHistory: {
        backgroundColor: '#444',
        borderRadius: '10px',
        padding: '10px 0 10px 0',
        overflowY: 'auto',
        flexGrow: 1,
        display: 'flex',
        flexDirection: 'column',
    },
    chatHistoryMessageAssistant:{
        backgroundColor: '#1dc2af',
        color: 'white',
        borderRadius: '10px 10px 10px 0',
        padding: '10px',
        margin: '0 0 20px 15px',
        maxWidth: '80%',
        alignSelf: 'flex-start',
    },
    chatHistoryMessageUser: {
        backgroundColor: '#6c6c6c',
        color: 'white',
        borderRadius: '10px 10px 0 10px',
        padding: '10px',
        margin: '0 15px 20px 0',
        maxWidth: '80%',
        alignSelf: 'flex-end',
    },
    chatHistoryMessageTime: {
        fontSize: '8pt',
        opacity: '0.5',
        marginTop: '-20px',
    },
    chatHistoryMessageTimeAssistant: {
        marginLeft: '15px',
    },
    chatHistoryMessageTimeUser: {
        marginRight: '15px',
        alignSelf: 'flex-end',
    },
    chatInput: {
        minHeight: '70px',
        backgroundColor: '#444',
        borderRadius: '10px',
        display: 'flex',
        alignItems: 'center',
        paddingLeft: '10px',
    },
    chatMessage: {
        textarea: {
            color: 'white'
        }
    }
}));

interface ChatProps {
    chatId: string;
}

function Chat({ chatId }: ChatProps) {
    const { classes } = useStyles();
    const bottomOfChatRef = useRef<HTMLDivElement>(null);

    const [messages, setMessages] = useState<ChatMessage[]>([]);
    const [message, setMessage] = useState<string>('');

    const sendMessage = (message: string) => {
        if (!message || message == '') return;

        const msg = { chatId: chatId, content: message, role: { label: 'user' }, timeStamp: new Date().toISOString() };
        setMessages([...messages, msg]);

        fetch(ApiUrls.Chat.SendMessage(chatId), {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ content: message })
        })
        .then(response => response.json())
        .then(data => {
            setMessages([...messages, msg, data]);
            setMessage('');
        });
    }

    function handleKeyPress(e: React.KeyboardEvent<HTMLDivElement>) {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            sendMessage(message);
        }
    }

    function convertDate(date: string): string {
        const d = new Date(date);
        const options: Intl.DateTimeFormatOptions = {
            year: 'numeric', month: '2-digit', day: '2-digit',
            hour: '2-digit', minute: '2-digit', hourCycle: 'h24'
        }
        return d.toLocaleString('de-DE', options);
    }

    useEffect(() => {
        if (chatId) {
            fetch(ApiUrls.Chat.GetMessages(chatId))
            .then(response => response.json())
            .then(data => setMessages(data));
        }
    }, [chatId]);
    useEffect(() => {
        if (bottomOfChatRef.current) {
            bottomOfChatRef.current.scrollIntoView({ behavior: 'smooth' });
        }
    }, [messages]);

    return (
        <Box className={classes.main}>
            <Box className={classes.chatHistory}>
                {messages.map((message, index) => {
                    if (message.role.label !== 'system')
                        return (
                            <>
                                <Box
                                    key={index}
                                    className={message.role.label === 'assistant' ? classes.chatHistoryMessageAssistant : classes.chatHistoryMessageUser}>
                                    {message.content}
                                </Box>
                                <div className={classes.chatHistoryMessageTime + ' ' + (message.role.label === 'assistant' ? classes.chatHistoryMessageTimeAssistant : classes.chatHistoryMessageTimeUser)}>
                                    {convertDate(message.timeStamp)}
                                </div>
                            </>
                        )
                })}
                <div ref={bottomOfChatRef}></div>
            </Box>
            <Box className={classes.chatInput}>
                <TextField 
                    id="chat-message"
                    multiline 
                    maxRows={2}
                    placeholder='Type message here...'
                    variant='standard'
                    fullWidth
                    className={classes.chatMessage}
                    value={message}
                    onChange={(e) => setMessage(e.target.value)}
                    onKeyUp={handleKeyPress}/>
                <IconButton onClick={() => { sendMessage(message); }}>
                    <SendIcon sx={{color: 'white'}} />
                </IconButton>
            </Box>
        </Box>
    )
}

export default React.memo(Chat);