using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{

        private Vector3 position;
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }


        private string type;
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public Block(Vector3 position, string type)
        {
            this.position = position;
            this.type = type;
        }


    
}
