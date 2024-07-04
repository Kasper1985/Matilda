export interface AuthorRole {
    label: string;
}

export interface Partition {
    text: string;
    relevance: number;
    partitionNumber: number;
    sectionNumber: number;
    lastUpdate: string;
    tags: { key: string, value: string }[];
}

export interface Citation {
    link: string;
    index: string;
    documentId: string;
    fileId: string;
    sourceContentType: string;
    sourceName: string;
    sourceUrl: string;
    partitions: Partition[];
}

export interface ChatMessage {
    id?: string;
    chatId: string;
    role: AuthorRole;
    timeStamp: string;
    content: string;
    citations?: Citation[];
    tokenUsage?: { key: string, value: number }[];
}