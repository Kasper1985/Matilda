import { useEffect, useRef, useState, useContext } from 'react';
import { Box } from '@mui/material';
import { AudioConfig, SpeechConfig, SpeechSynthesizer, ResultReason } from 'microsoft-cognitiveservices-speech-sdk';
import { MessageInputContext } from '../contexts/MessageInputContextProvider';
import { ChatMessage } from '../models';
import { ApiUrls } from '../api/apiUrls';
import { configuration } from '../configuration';
import { chatHistoryStyles } from './styles/ChatHistory.styles';

interface ChatHistoryProps {
    chatId: string;
}

export function ChatHistory({ chatId }: ChatHistoryProps) {
    const {classes} = chatHistoryStyles();
    const bottomOfChatRef = useRef<HTMLSpanElement>(null);
    const audioConfig = AudioConfig.fromDefaultSpeakerOutput();
    if (!configuration.azureSpeech?.SUBSCRIPTION_KEY || !configuration.azureSpeech?.REGION) throw new Error('Azure Speech configuration is missing.');
    const speechConfig = SpeechConfig.fromSubscription(configuration.azureSpeech.SUBSCRIPTION_KEY, configuration.azureSpeech.REGION);
    const synthesizer = new SpeechSynthesizer(speechConfig, audioConfig);


    const [messages, setMessages] = useState<ChatMessage[]>([]);
    const [answer, setAnswer] = useState<ChatMessage>();
    const [answerText, setAnswerText] = useState<string>('');

    const [messageInput] = useContext(MessageInputContext);

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

    useEffect(() => {
        if (!messageInput.message || messageInput.message === '') return;
        if (!chatId || chatId === '') return;
        sendMessage(messageInput.message);
    }, [messageInput]);

    useEffect(() => {
        if (!answer) return;
        setMessages([...messages, answer]);
        synthesize(answer.content);
        const tokens = answer.content.split(' ');
        const interval = setInterval(() => {
            if (tokens.length > 0) {
                setAnswerText((prev) => `${prev} ${tokens.shift()}`);
                bottomOfChatRef.current!.scrollIntoView({ behavior: 'smooth' });
            } else {
                clearInterval(interval);
                setAnswerText('');
                setAnswer(undefined);
            }
        }, 300);
    }, [answer]);

    function sendMessage(message: string) {
        const msg = { id: '-1', chatId: chatId, content: message, role: { label: 'user' }, timeStamp: new Date().toISOString() };
        setMessages([...messages, msg]);

        fetch(ApiUrls.Chat.SendMessage(chatId), {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ content: message })
        })
        .then(response => response.json())
        .then(data => setAnswer(data));
    }

    function convertDate(date: string) {
        const d = new Date(date);
        const options: Intl.DateTimeFormatOptions = {
            year: 'numeric', month: '2-digit', day: '2-digit',
            hour: '2-digit', minute: '2-digit', hourCycle: 'h24'
        }
        return d.toLocaleString('de-DE', options);
    }

    function synthesize(text: string) {
        synthesizer.speakTextAsync(text,
            (result) => {
                if (result.reason === ResultReason.SynthesizingAudioCompleted) {
                    //console.log('Speech synthesis completed');
                } else {
                    console.error('Speech synthesis canceled, error: ' + result.errorDetails);
                }
                synthesizer.close();
            },
            (error) => {
                console.trace('err - ' + error);
                synthesizer.close();
            });
    }

    return (
        <Box className={classes.container}>
            <Box className={classes.elements}>
                {messages.map((message, index) => {
                    return (
                        <Box key={index} className={classes[message.role.label]}>
                            <Box>{convertDate(message.timeStamp)}</Box>
                            <Box>{message.id === answer?.id ? answerText : message.content}</Box>
                        </Box>
                    )})}
                <span ref={bottomOfChatRef}/>
            </Box>
        </Box>
    );
}