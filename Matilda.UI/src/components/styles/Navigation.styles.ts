import { makeStyles } from 'tss-react/mui';

export const navigationStyles: any = makeStyles()(() => ({
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
    }
}));