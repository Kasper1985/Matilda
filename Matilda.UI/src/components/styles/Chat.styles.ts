import { makeStyles } from 'tss-react/mui';

export const chatStyles: any = makeStyles()(() => ({
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
        alignSelf: 'flex-start'
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
    },
    message: {
        display: 'flex',
        flexDirection: 'column',
    }
}));