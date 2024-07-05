import React, { useState, useEffect, useRef } from 'react';
import { chatStyles } from './styles/Chat.styles.ts';
import { Box, IconButton, TextField } from '@mui/material';
import { ChatMessage } from '../../api/models';
import { ApiUrls } from '../../api/apiUrls';
import SendIcon from '@mui/icons-material/Send';

interface ChatProps {
    chatId: string;
}

function Chat({ chatId }: ChatProps) {
    const { classes } = chatStyles();
    const bottomOfChatRef = useRef<HTMLDivElement>(null);
    const [answer, setAnswer] = useState<ChatMessage>(); 
    const [text, setText] = useState<string>('');
    
    const [messages, setMessages] = useState<ChatMessage[]>([]);
    const [message, setMessage] = useState<string>('');

    useEffect(() => {
        if (!answer) return;
        setText('');
        const tokens = answer.content.split(' ');
        const interval = setInterval(() => {
            if (tokens.length > 0) {
                setText((prev) => prev + ' ' + tokens.shift());
                bottomOfChatRef.current!.scrollIntoView({ behavior: 'smooth' });
            }
        }, 200);

        return () => clearInterval(interval);
    }, [answer]);

    useEffect(() => {
        if (chatId) {
            fetch(ApiUrls.Chat.GetMessages(chatId))
            .then(response => response.json())
            .then(data => setMessages(data));
        } else {
            setMessages([]);
        }
    }, [chatId]);

    useEffect(() => {
        if (bottomOfChatRef.current) {
            bottomOfChatRef.current.scrollIntoView({ behavior: 'smooth' });
        }
    }, [messages]);

    function sendMessage(message: string) {
        if (!message || message == '') return;

        const msg = { chatId: chatId, content: message, role: { label: 'user' }, timeStamp: new Date().toISOString() };
        setMessage('');
        setMessages([...messages, msg]);

        fetch(ApiUrls.Chat.SendMessage(chatId), {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ content: message })
        })
        .then(response => response.json())
        .then(data => 
            {
                setMessages([...messages, msg, data]);
                setAnswer(data);
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

    return (
        <Box className={classes.main}>
            <Box className={classes.chatHistory}>
                {messages.map((message, index) => {
                    if (message.role.label !== 'system')
                        return (
                            <Box key={index} className={classes.message}>
                                <Box
                                    className={message.role.label === 'assistant' ? classes.chatHistoryMessageAssistant : classes.chatHistoryMessageUser}>
                                    {message.id && answer?.id && message.id === answer.id ? text : message.content}
                                </Box>
                                <Box
                                    className={classes.chatHistoryMessageTime + ' ' + (message.role.label === 'assistant' ? classes.chatHistoryMessageTimeAssistant : classes.chatHistoryMessageTimeUser)}>
                                    {convertDate(message.timeStamp)}
                                </Box>
                            </Box>
                        )
                })}
                <Box ref={bottomOfChatRef}/>
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