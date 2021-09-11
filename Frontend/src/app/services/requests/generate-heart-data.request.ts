import { DataGenerationRate } from "src/app/models/data-generation-rate.enum";

export class GenerateHeartDataRequest {
    Max: number;
    Step: number;
    Rate: DataGenerationRate;
    ConnectionId: string;

    constructor(max: number, step: number, rate: DataGenerationRate, connectionId: string) {
        this.Max = max;
        this.Step = step;
        this.Rate = rate;
        this.ConnectionId = connectionId;
    }
}
