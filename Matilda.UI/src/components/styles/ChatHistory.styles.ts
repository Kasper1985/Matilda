import { makeStyles } from "tss-react/mui";

export const chatHistoryStyles: any = makeStyles()(() => ({
    container: {
        padding: '10px 5px 10px 0',
        backgroundColor: '#444',
        borderRadius: '10px',
        height: '100%',
        overflowY: 'hidden',
    },
    elements: {
        display: 'flex',
        flexDirection: 'column',
        overflowY: 'auto',
        height: '100%',
    },
    system: {
        display: 'none',
    },
    assistant: {
        maxWidth: '80%',
        alignSelf: 'flex-start',

        '*:first-of-type': {
            fontSize: '8pt',
            opacity: '0.5',
            marginLeft: '20px',
        },

        '*:last-child': {
            backgroundColor: '#1dc2af',
            color: 'white',
            borderRadius: '10px 10px 10px 0',
            padding: '10px',
            margin: '0 0 20px 15px',
            width: 'fit-content',
        }
    },
    user: {
        maxWidth: '80%',
        alignSelf: 'flex-end',
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'flex-end',

        '*:first-of-type': {
            fontSize: '8pt',
            opacity: '0.5',
            marginRight: '20px',
        },

        '*:last-child': {
            backgroundColor: '#6c6c6c',
            color: 'white',
            borderRadius: '10px 10px 0 10px',
            padding: '10px',
            margin: '0 15px 20px 0',
            width: 'fit-content',
        }
    },
}));

