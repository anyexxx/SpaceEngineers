﻿using Havok;
using Sandbox.Common.Components;
using Sandbox.Engine.Physics;
using Sandbox.Game.Components;
using Sandbox.Game.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using VRage.Import;
using VRageMath;
using VRageMath.PackedVector;
using VRageRender;

namespace Sandbox.Game.Components
{
    public class MyRenderComponentFracturedPiece : MyRenderComponent
    {
        struct ModelInfo
        {
            public String Name;
            public MatrixD LocalTransform;
        }

        readonly List<ModelInfo> Models = new List<ModelInfo>();

        public void AddPiece(string modelName, MatrixD localTransform)
        {
            if (string.IsNullOrEmpty(modelName))
                modelName = @"Models\Debug\Error.mwm";
            Models.Add(new ModelInfo() { Name = modelName, LocalTransform = localTransform });
        }

        public override void InvalidateRenderObjects(bool sortIntoCullobjects = false)
        {
            // Update only cull object
            var m = Entity.PositionComp.WorldMatrix;
            if ((Entity.Visible || Entity.CastShadows) && Entity.InScene && Entity.InvalidateOnMove && m_renderObjectIDs.Length > 0)
            {
                VRageRender.MyRenderProxy.UpdateRenderObject(m_renderObjectIDs[0], ref m, sortIntoCullobjects);
            }
        }

        public override void AddRenderObjects()
        {
            if (Models.Count == 0)
                return;

            m_renderObjectIDs = new uint[Models.Count + 1];

            m_renderObjectIDs[0] = VRageRender.MyRenderProxy.RENDER_ID_UNASSIGNED;
            SetRenderObjectID(0, MyRenderProxy.CreateManualCullObject(this.Entity.Name ?? "Fracture", this.Entity.PositionComp.WorldMatrix));

            for (int i = 0; i < Models.Count; ++i)
            {
                m_renderObjectIDs[i + 1] = VRageRender.MyRenderProxy.RENDER_ID_UNASSIGNED;
                SetRenderObjectID(i + 1, MyRenderProxy.CreateRenderEntity
                (
                    "Fractured piece " + i.ToString() + " " + this.Entity.EntityId.ToString(),
                    Models[i].Name,
                    Models[i].LocalTransform,
                    MyMeshDrawTechnique.MESH,
                    GetRenderFlags(),
                    GetRenderCullingOptions(),
                    m_diffuseColor,
                    m_colorMaskHsv
                ));

                MyRenderProxy.SetParentCullObject(m_renderObjectIDs[i + 1], m_renderObjectIDs[0], Models[i].LocalTransform);
            }
        }

        public void ClearModels()
        {
            Models.Clear();
        }
    }
}
