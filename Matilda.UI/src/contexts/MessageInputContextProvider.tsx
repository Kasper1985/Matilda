import { createContext, useContext, ReactNode, useState } from 'react';
import { DataContextType } from '../utils';

export interface MessageInput {
    message: string;
}

const stateMessageInputInitial: MessageInput = { message: '' };

export const MessageInputContext = createContext<DataContextType<MessageInput>>([
    stateMessageInputInitial,
    undefined
] as unknown as DataContextType<MessageInput>);

export function MessageInputProvider(props: { children: ReactNode }) {
    const [data, setData] = useState(stateMessageInputInitial);

    return <MessageInputContext.Provider value={[data, setData]}>{props.children}</MessageInputContext.Provider>;
}

export function useMessageInput() {
    const [messageInput, setMessageInput] = useContext(MessageInputContext);
    
    function setMessageInputEx(data: Partial<MessageInput>) {
        const newData = { ...messageInput, ...data };
        setMessageInput(newData);
    }
    return { messageInput, setMessageInput: setMessageInputEx };
}