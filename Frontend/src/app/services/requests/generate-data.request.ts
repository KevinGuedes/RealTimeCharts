import { DataGenerationRate } from "src/app/models/data-generation-rate.enum";
import { DataType } from "src/app/models/data-type.enum";

export class GenerateDataRequest {
    Max: number;
    Step: number;
    Rate: DataGenerationRate;
    ConnectionId: string;
    DataType: DataType;

    constructor(max: number, step: number, rate: DataGenerationRate, dataType: DataType, connectionId: string) {
        this.Max = max;
        this.Step = step;
        this.Rate = rate;
        this.DataType = dataType;
        this.ConnectionId = connectionId;
    }
}
