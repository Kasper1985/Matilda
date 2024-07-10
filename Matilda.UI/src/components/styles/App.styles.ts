import { makeStyles } from 'tss-react/mui';

export const appStyles: any = makeStyles()(() => ({
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
        display: 'flex',
        flexDirection: 'column',
        gap: '10px'
    }
}));