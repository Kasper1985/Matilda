import { configuration } from '../configuration';

export type ApiUrl = string;

function getApiChat(baseUrl: string) {
    const serviceName = '';
    const servicePath = `${baseUrl}${serviceName ? '/' + serviceName : ''}`;
    
    return {
        /// GET
        GetAllChats: `${servicePath}/Chat`,
        GetChat: (id: string) => `${servicePath}/Chat/${id}`,
        GetMessages: (id: string) => `${servicePath}/Chat/${id}/messages`,
        /// POST
        CreateChat: (title: string) => `${servicePath}/Chat/${title}`,
        SendMessage: (id: string) => `${servicePath}/Chat/${id}/ask`,
        /// DELETE
        DeleteChat: (id: string) => `${servicePath}/Chat/${id}`,
    };
}

function getSpeechService(baseUrl: string) {
    const serviceName = '';
    const servicePath = `${baseUrl}${serviceName ? '/' + serviceName : ''}`;

    return {
        GetTextToSpeech: (text: string) => `${servicePath}/Speech/TextToSpeech?text=${text}`,
    }
}

export const ApiUrls = {
    Chat: getApiChat(configuration.env.SERVICE_BASE_URL),
    Speech: getSpeechService(configuration.env.SERVICE_BASE_URL),
}