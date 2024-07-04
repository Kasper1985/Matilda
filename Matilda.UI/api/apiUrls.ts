import config from './configuration';

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

export const ApiUrls = {
    Chat: getApiChat(config.env.SERVICE_BASE_URL),
}