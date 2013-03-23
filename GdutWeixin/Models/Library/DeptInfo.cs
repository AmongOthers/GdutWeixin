using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace GdutWeixin.Models.Library
{
    public class DeptInfo
    {
        static DeptInfoBuilder sBuilder = new DeptInfoBuilder();

        public static List<DeptInfo> Build(string content)
        {
            return sBuilder.Build(content);
        }

        public string DeptPlace { get; set; }

        public string Index { get; set; }

        public string Register { get; set; }

        public string Volume { get; set; }

        public string Year { get; set; }

		public const string Available = "available";
		public const string Lent = "lent";
		public const string ReadOnly = "readonly";

        private string mStatusCode;
        public string StatusCode
        {
            get
            {
                return mStatusCode;
            }
            set { }
        }

        private string mStatus;
        public string Status
        {
            get
            {
                return mStatus;
            }
            set
            {
                mStatus = value;
                if (mStatus == "可供出借")
                {
                    mStatusCode = Available;
                }
                else if (mStatus == "仅供阅览")
                {
                    mStatusCode = ReadOnly;
                }
                else
                {
                    mStatusCode = Lent;
                }
            }
        }

        public string Type { get; set; }

        class DeptInfoBuilder
        {
            List<DeptInfo> mDeptInfos;
            XmlReader mReader;
            DeptInfoBuilderState mState;

            enum DeptInfoBuilderState
            {
                DeptPlace,
                Index,
                Register,
                Volume,
                Year,
                Status,
                Type,
                Other
            }

            public List<DeptInfo> Build(string content)
            {
				//reset
                mDeptInfos = new List<DeptInfo>();

                using (mReader = XmlReader.Create(new MemoryStream(Encoding.UTF8.GetBytes(content))))
                {
                    DeptInfo deptInfo = null;

                    while (mReader.Read())
                    {
                        if (mReader.NodeType == XmlNodeType.Element)
                        {
                            if (mReader.Name == "tr")
                            {
								deptInfo = new DeptInfo();
								mDeptInfos.Add(deptInfo);
								mState = DeptInfoBuilderState.DeptPlace;
                            }
                            else if (mReader.Name == "a")
                            {
                                deptInfo.DeptPlace = mReader.ReadString().Trim();
								mState = DeptInfoBuilderState.Index;
                            }
                            else if (mReader.Name == "td")
                            {
                                switch (mState)
                                {
                                    case DeptInfoBuilderState.Index:
                                        deptInfo.Index = mReader.ReadString().Trim();
                                        mState = DeptInfoBuilderState.Register;
                                        break;
                                    case DeptInfoBuilderState.Register:
                                        deptInfo.Register = mReader.ReadString().Trim();
                                        mState = DeptInfoBuilderState.Volume;
                                        break;
                                    case DeptInfoBuilderState.Volume:
                                        deptInfo.Volume = mReader.ReadString().Trim();
                                        mState = DeptInfoBuilderState.Year;
                                        break;
                                    case DeptInfoBuilderState.Year:
                                        deptInfo.Year = mReader.ReadString().Trim();
                                        mState = DeptInfoBuilderState.Status;
                                        break;
                                    case DeptInfoBuilderState.Status:
                                        deptInfo.Status = mReader.ReadString().Trim();
                                        mState = DeptInfoBuilderState.Type;
                                        break;
                                    case DeptInfoBuilderState.Type:
                                        deptInfo.Type = mReader.ReadString().Trim();
                                        mState = DeptInfoBuilderState.Other;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
                return mDeptInfos;
            }
        }
    }
}