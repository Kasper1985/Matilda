export enum EnvName {
    LOCAL = 'LOCAL',
    DEV = 'DEV',
}
export interface Environment {
    NAME: EnvName;
    SERVICE_BASE_URL: string;
}
export interface AzureSpeech {
    SUBSCRIPTION_KEY: string;
    REGION: string;
}
export interface EnvSettings {}


export interface Config {
    name: string;
    env: Environment;
    azureSpeech?: AzureSpeech;
    settings?: EnvSettings;
}

const config: Config = {
    name: 'default',
    env: {
        NAME: process.env.APP_ENV as EnvName || EnvName.LOCAL,
        SERVICE_BASE_URL: process.env.SERVICE_BASE_URL ?? '',
    },
    azureSpeech: {
        SUBSCRIPTION_KEY: process.env.AZURE_SPEECH_KEY ?? '',
        REGION: process.env.AZURE_SPEECH_REGION ?? '',
    },
}

export const configuration = {
    ...config,
}