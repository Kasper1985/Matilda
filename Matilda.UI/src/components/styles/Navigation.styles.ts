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
    chatListContainer: {
        backgroundColor: '#444',
        borderRadius: '10px',
        padding: '10px 0',
        overflowY: 'hidden',
        flexGrow: 1,
        display: 'flex',
        flexDirection: 'column'
    },
    chatListButtons: {
        display: 'flex',
        justifyContent: 'center',
        gap: '10px',
    },
    chatList: {
        marginRight: '5px',
        overflowY: 'auto',
    }
}));