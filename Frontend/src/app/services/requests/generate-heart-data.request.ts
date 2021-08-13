import { DataGenerationRate } from "src/app/models/data-generation-rate.enum";

export class GenerateHeartDataRequest {
    Max: number;
    Step: number;
    Rate: DataGenerationRate;

    constructor(max: number, step: number, rate: DataGenerationRate) {
        this.Max = max;
        this.Step = step;
        this.Rate = rate;
    }
}
