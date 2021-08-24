//-----------------------------------------------------------------------
// <copyright file="PeriodicUpdateUnit.cs" company="Lost Signal">
//     Copyright (c) Lost Signal. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_VISUAL_SCRIPTING

namespace Lost
{
    using Unity.VisualScripting;

    [UnitCategory("Events/Lifecycle")]
    [UnitTitle("Periodic Update")]
    public sealed class PeriodicUpdateUnit : EventUnit<EmptyEventArgs>
    {
        private GraphReference graphReference;
        private float deltaTime;

        [DoNotSerialize] public ValueInput CallPerSecond { get; private set; }

        [DoNotSerialize] public ValueOutput DeltaTime { get; private set; }

        protected override bool register => false;

        public override void Instantiate(GraphReference instance)
        {
            base.Instantiate(instance);
            this.graphReference = instance;
            UpdateManager.Instance.RegisterFunction(this.UpdateUnit, Flow.New(instance).GetValue<int>(this.CallPerSecond));
        }

        public override void Uninstantiate(GraphReference instance)
        {
            base.Uninstantiate(instance);
            UpdateManager.Instance.UnregisterFunction(this.UpdateUnit);
        }

        protected override void Definition()
        {
            base.Definition();

            this.CallPerSecond = this.ValueInput<int>("Calls Per Sec");
            this.DeltaTime = this.ValueOutput("Delta Time", this.GetDeltaTime).Predictable();
        }

        private void UpdateUnit(float deltaTime)
        {
            this.deltaTime = deltaTime;
            this.Trigger(this.graphReference, default(EmptyEventArgs));
        }

        private float GetDeltaTime(Flow flow) => this.deltaTime;
    }
}

#endif
