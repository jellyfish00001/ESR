import { JudgeNullPipe } from './judge-null.pipe';

describe('JudgeNullPipe', () => {
  it('create an instance', () => {
    const pipe = new JudgeNullPipe();
    expect(pipe).toBeTruthy();
  });
});
