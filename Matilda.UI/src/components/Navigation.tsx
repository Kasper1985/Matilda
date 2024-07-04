import React, { useState, useEffect } from 'react';
import { makeStyles } from 'tss-react/mui';
import { Box, Button, List, ListItem, ListItemButton, ListItemIcon, ListItemText, IconButton } from '@mui/material';
import ChatIcon from '@mui/icons-material/Chat';
import DeleteIcon from '@mui/icons-material/Delete';
import avatar from '../assets/avatar.jpg';
import { ApiUrls } from '../../api/apiUrls';
import { Chat } from '../../api/models';

const useStyles = makeStyles()(() => ({
    main: {
        height: '100%',
        display: 'flex',
        flexDirection: 'column',
        gap: '10px',
    },
    avatarBox: {
        backgroundColor: '#444',
        borderRadius: '10px',
        padding: '10px 0 10px 0',
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        ' img': {
            width: '120px',
            height: '120px',
            borderRadius: '50%',
        }
    },
    chatList: {
        backgroundColor: '#444',
        borderRadius: '10px',
        padding: '10px 0 10px 0',
        overflowY: 'auto',
        flexGrow: 1,
        display: 'flex',
        flexDirection: 'column'
    },
    chatListItem: {
        '&:hover': {
            backgroundColor: 'rgba(255, 255, 255, 0.4)'
        }
    },
}));

interface NavigationProps {
    onSelectChat?: (id: string) => void;
}

function Navigation({ onSelectChat }: NavigationProps) {
    const { classes } = useStyles();

    const [chats, setChats] = useState<Chat[]>([]);
    useEffect(() => LoadChats(), []);

    const LoadChats = () => {
        fetch(ApiUrls.Chat.GetAllChats)
        .then(response => response.json())
        .then(data => setChats(data))
    }

    const CreateNewChat = (title: string) => {
        fetch(ApiUrls.Chat.CreateChat(title), {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ title: title })
        })
        .then(response => response.json())
        .then(data => {
            setChats([...chats, data]);
            onSelectChat && onSelectChat(data.id);
        });
    }
    
    return (
        <Box className={classes.main}>
            <Box className={classes.avatarBox}>
                <img src={avatar} alt="avatar"/>
                <Box sx={{ fontSize: '20pt', fontWeight: 'bold' }}>Matilda</Box>
                <Box sx={{ fontSize: '14pt', opacity: '50%'}}>Personal AI Assistant</Box>
            </Box>
            <Box className={classes.chatList}>
                <Button variant="text" sx={{color: '#c7c7c7', fontWeight: '600'}} onClick={() => CreateNewChat('New chat')}>New Chat</Button>
                <List>
                    {chats.map((chat) => (
                        <ListItem 
                            key={chat.id}
                            secondaryAction={
                                <IconButton edge="end" aria-label="delete">
                                    <DeleteIcon sx={{color: 'white'}}/>
                                </IconButton>
                            }
                            className={classes.chatListItem} disablePadding>
                                <ListItemButton onClick={() => onSelectChat && onSelectChat(chat.id)}>
                                    <ListItemIcon>
                                        <ChatIcon sx={{color: 'white'}}/>
                                    </ListItemIcon>
                                    <ListItemText primary={chat.title}/>
                                </ListItemButton>
                        </ListItem>
                    ))}
                </List>
            </Box>
        </Box>
    )
}

export default React.memo(Navigation);