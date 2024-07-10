import { useState, useEffect, FormEvent } from 'react';
import { navigationStyles } from './styles/Navigation.styles.ts';
import { Box, Button, List, ListItem, ListItemButton, ListItemIcon, ListItemText, IconButton, Dialog, DialogTitle, DialogContent, DialogContentText, TextField, DialogActions } from '@mui/material';
import { ApiUrls } from '../api/apiUrls';
import { ChatHistory } from '../models/index.ts';
import { Refresh, Chat, Delete } from '@mui/icons-material';
import avatar from '../assets/avatar.jpg';

interface NavigationProps {
    onSelectChat?: (id: string) => void;
}

export function Navigation({ onSelectChat }: NavigationProps) {
    const { classes } = navigationStyles();

    const [chats, setChats] = useState<ChatHistory[]>([]);
    const [isOpenNewChatDialog, setIsOpenNewChatDialog] = useState(false);
    const [selectedChatId, setSelectedChatId] = useState('');

    useEffect(() => LoadChats(), []);

    function LoadChats() {
        fetch(ApiUrls.Chat.GetAllChats)
        .then(response => response.json())
        .then(data => setChats(data))
    }

    function CreateNewChat(title: string) {
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
            setSelectedChatId(data.id);
            onSelectChat && onSelectChat(data.id);
        });
    }

    function DeleteChat(id: string) {
        fetch(ApiUrls.Chat.DeleteChat(id), {
            method: 'DELETE'
        })
        .then(response => {
            if (!response.ok) {
                alert("Failed to delete chat.");
                console.error("Failed to delete chat with id: " + id);
                return;
            }

            const newChats = chats.filter(chat => chat.id !== id);
            setChats(newChats);
            if (selectedChatId === id)
            {
                if (newChats.length > 0) {
                    setSelectedChatId(newChats[0].id);
                    onSelectChat && onSelectChat(newChats[0].id);
                } else {
                    setSelectedChatId('');
                    onSelectChat && onSelectChat('');
                }
            }
        });
    }
    
    return (
        <Box className={classes.main}>
            <Box className={classes.avatarBox}>
                <img src={avatar} alt="avatar"/>
                <Box sx={{ fontSize: '20pt', fontWeight: 'bold' }}>Matilda</Box>
                <Box sx={{ fontSize: '14pt', opacity: '50%'}}>Personal AI Assistant</Box>
            </Box>
            <Box className={classes.chatListContainer}>
                <Box className={classes.chatListButtons}>
                    <Button variant="outlined" sx={{ width: 'fit-content', alignSelf: 'center' }} onClick={() => setIsOpenNewChatDialog(true)}>
                        New Chat
                    </Button>
                    <IconButton onClick={LoadChats}>
                        <Refresh />
                    </IconButton>
                </Box>
                <Dialog 
                    open={isOpenNewChatDialog}
                    onClose={() => setIsOpenNewChatDialog(false)}
                    PaperProps={{
                        component: 'form',
                        onSubmit: (e: FormEvent<HTMLFormElement>) => {
                            e.preventDefault();
                            const formData = new FormData(e.currentTarget);
                            const formJson = Object.fromEntries((formData as any).entries());
                            CreateNewChat(formJson.title);
                            setIsOpenNewChatDialog(false);
                        }
                    }}>
                    <DialogTitle>New Chat</DialogTitle>
                    <DialogContent>
                        <DialogContentText>Enter the title of the new chat.</DialogContentText>
                        <TextField autoFocus required margin='dense' id='title' name='title' label='Title' type='text' fullWidth variant='standard'/>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => setIsOpenNewChatDialog(false)}>Cancel</Button>
                        <Button type='submit'>Create</Button>
                    </DialogActions>
                </Dialog>
                <List className={classes.chatList}>
                    {chats.map((chat) => (
                        <ListItem 
                            key={chat.id}
                            secondaryAction={
                                <IconButton edge="end" aria-label="delete" onClick={() => DeleteChat(chat.id)}>
                                    <Delete />
                                </IconButton>
                            }
                            disablePadding>
                                <ListItemButton selected={chat.id === selectedChatId} onClick={() => {
                                    setSelectedChatId(chat.id);
                                    onSelectChat && onSelectChat(chat.id);
                                }}>
                                    <ListItemIcon>
                                        <Chat />
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