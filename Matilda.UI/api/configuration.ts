export enum EnvName {
    LOCAL = 'LOCAL',
    DEV = 'DEV',
}
export interface Environment {
    NAME: EnvName;
    SERVICE_BASE_URL: string;
}
export interface EnvSettings {}

export interface Config {
    name: string;
    env: Environment;
    settings: EnvSettings;
}

const config: Config = {
    name: 'default',
    settings: {},
    env: {
        NAME: EnvName.LOCAL,
        SERVICE_BASE_URL: 'http://localhost:5167/api',
    }
}

export default {
    ...config
}