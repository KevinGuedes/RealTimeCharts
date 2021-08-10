export class GenerateHeartDataRequest {
    Max: number;
    Step: number;

    constructor(max: number, step: number) {
        this.Max = max;
        this.Step = step;
    }
}
